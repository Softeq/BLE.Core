using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Softeq.BLE.Core.Result;
using Softeq.BLE.Core.Utils.Extensions;

namespace Softeq.BLE.Core.BleDevice.Core
{
    internal sealed partial class BleDeviceBase<T>
    {
        public Task<IBleResult> TryConnectAsync(CancellationToken cancellationToken)
        {
            return _bleInfrastructure.BleAvailability.ExecuteWithBleAvailabilityCheckAsync(() => ConnectWithPolicyAsync(false, cancellationToken));
        }

        public Task<IBleResult> TryAutoConnectAsync(CancellationToken cancellationToken)
        {
            return _bleInfrastructure.BleAvailability.ExecuteWithBleAvailabilityCheckAsync(() => ConnectWithPolicyAsync(true, cancellationToken));
        }

        public async Task<IBleResult> TryDisconnectAsync()
        {
            IBleResult result;

            if (IsConnected)
            {
                result = await _bleInfrastructure.ConnectionManager.TryDisconnectFromDeviceAsync(_device).ConfigureAwait(false);
            }
            else
            {
                _connectionCancellation?.Cancel();
                _connectionCancellation = null;
                result = BleResult.Success();
            }

            return result;
        }

        private async Task<IBleResult> DoConnectToDeviceAsync(bool connectWhenAvailable, CancellationToken cancellationToken)
        {
            IBleResult result;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await _connectionSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                _bleInfrastructure.Logger.Log(LogSender, $"Semaphore equired after {stopwatch.Elapsed}");
                try
                {
                    _connectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    result = await _bleInfrastructure.ConnectionManager.TryConnectToDeviceAsync(_device, connectWhenAvailable, _connectionCancellation.Token);
                    _bleInfrastructure.Logger.Log(LogSender, $"Connected after {stopwatch.Elapsed}");
                }
                finally
                {
                    _connectionCancellation = null;
                    _connectionSemaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure(BleFailure.OperationCancelled);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }
            finally
            {
                stopwatch.Stop();
            }

            return result;
        }

        private async Task<IBleResult> InitializeProtocolAsync(CancellationToken cancellationToken)
        {
            IBleResult result;

            try
            {
                var failedResults = new List<IBleResult>();
                foreach (var characteristic in _deviceCharacteristics.Values)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var initializationResult = await characteristic.InitializeAsync(_device, cancellationToken);
                    if (!initializationResult.IsOperationCompleted)
                    {
                        failedResults.Add(initializationResult);
                    }
                }
                result = failedResults.Any() ? BleResult.Failure(failedResults.ToArray()) : BleResult.Success();
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure(BleFailure.OperationCancelled);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }

            return result;
        }

        private async Task<IBleResult> ConnectWithPolicyAsync(bool connectWhenAvailable, CancellationToken cancellationToken)
        {
            IBleResult result;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var deviceSearchResult = await EnsureDeviceIsFoundAsync(cancellationToken).ConfigureAwait(false);
                _bleInfrastructure.Logger.Log(LogSender, $"Search completed after {stopwatch.Elapsed}");
                if (deviceSearchResult.IsOperationCompleted)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var deviceConnectedResult = await EnsureDeviceIsConnectedAsync(connectWhenAvailable, cancellationToken);
                    _bleInfrastructure.Logger.Log(LogSender, $"Connection completed after {stopwatch.Elapsed}");
                    if (deviceConnectedResult.IsOperationCompleted)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var initializationResult = await InitializeProtocolAsync(cancellationToken);
                        _bleInfrastructure.Logger.Log(LogSender, $"Initialization completed after {stopwatch.Elapsed}");
                        result = initializationResult.IsOperationCompleted ? BleResult.Success() : BleResult.Failure(BleFailure.DeviceNotInitialized, initializationResult);
                    }
                    else
                    {
                        result = deviceConnectedResult;
                    }
                }
                else
                {
                    result = deviceSearchResult;
                }
            }
            catch (OperationCanceledException)
            {
                result = BleResult.Failure(BleFailure.OperationCancelled);
            }
            catch (Exception e)
            {
                result = BleResult.Failure(e);
            }
            finally
            {
                stopwatch.Stop();
            }

            return result;
        }

        private async Task<IBleResult> EnsureDeviceIsFoundAsync(CancellationToken cancellationToken)
        {
            IBleResult result;

            if (IsFound)
            {
                result = BleResult.Success();
            }
            else
            {
                var searchResult = await _bleInfrastructure.SearchManager.SearchForFirstDeviceAsync(_deviceSearchFilter, cancellationToken).ConfigureAwait(false);
                if (searchResult.IsOperationCompleted)
                {
                    _device = searchResult.Data;
                    result = _device != null ? BleResult.Success() : BleResult.Failure(BleFailure.DeviceNotFound);
                }
                else
                {
                    result = BleResult.Failure(BleFailure.DeviceNotFound, searchResult);
                }
            }

            return result;
        }

        private async Task<IBleResult> EnsureDeviceIsConnectedAsync(bool connectWhenAvailable, CancellationToken cancellationToken)
        {
            IBleResult result;

            if (IsConnected)
            {
                result = BleResult.Success();
            }
            else
            {
                var connectionResult = await DoConnectToDeviceAsync(connectWhenAvailable, cancellationToken).ConfigureAwait(false);

                result = connectionResult.IsOperationCompleted ? BleResult.Success() : BleResult.Failure(BleFailure.CannotConnect, connectionResult);
            }

            return result;
        }
    }
}
