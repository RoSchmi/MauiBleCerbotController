using Bluetooth;
using Bluetooth.Abstractions;
using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;
using Bluetooth.Abstractions.Scanning.Options;
using Bluetooth.Core;
using Bluetooth.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBleCerbotController.Infrastructure;
using MauiBleCerbotController.Interfaces;
using Microsoft.Extensions.Logging;
using Plugin.BaseTypeExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
//using static Android.Media.MediaDrm;
using static Microsoft.Maui.ApplicationModel.Permissions;
#if IOS
using CoreBluetooth;
#endif

namespace MauiBleCerbotController.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {

        private const int MinRssiValue = -100;
        private const int MaxRssiValue = -30;

        private IReadOnlyList<IBluetoothRemoteDevice> _allDevices = [];

        private readonly ILogger<MainPageViewModel> _logger;

        private readonly INavigationService _navigation;

        private readonly IBluetoothScanner _scanner;

      //  BluetoothScanner _BleScanner;

       // public IEnumerable<IBluetoothRemoteDevice>? Devices { get; }

        /***************************************************************************************************/


        /// <summary>
        ///     Collection of discovered Bluetooth devices.
        ///     Automatically updated when devices are discovered or removed.
        /// </summary>
        public ObservableCollection<IBluetoothRemoteDevice> Devices { get; } = new ObservableCollection<IBluetoothRemoteDevice>();

        /// <summary>
        ///     Gets or sets the currently selected Bluetooth device.
        /// </summary>
        public IBluetoothRemoteDevice? SelectedDevice
        {
            get => GetValue<IBluetoothRemoteDevice?>(null);
            private set => SetValue(value);
        }

        /// <summary>
        ///     Gets a value indicating whether scanning is currently active.
        /// </summary>
        public bool IsScanning
        {
            get => GetValue(false);
            private set => SetValue(value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether nameless devices are hidden.
        /// </summary>
        public bool HideUnnamedDevices
        {
            get => GetValue(true);
            set
            {
                if (SetValue(value))
                {
                   // ApplyFilters();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the minimum RSSI threshold (dBm) for displayed devices.
        /// </summary>
        public int MinimumSignalStrengthDbm
        {
            get => GetValue(MinRssiValue);
            set
            {
                var clamped = Math.Clamp(value, MinRssiValue, MaxRssiValue);
                if (SetValue(clamped))
                {
                    OnPropertyChanged(nameof(MinimumSignalStrengthText));
                  //  ApplyFilters();
                }
            }
        }

        /// <summary>
        ///     Gets or sets a case-insensitive name substring filter.
        /// </summary>
        public string NamePattern
        {
            get => GetValue(string.Empty);
            set
            {
                if (SetValue(value ?? string.Empty))
                {
                   ApplyFilters();
                }
            }
        }

        /// <summary>
        ///     Gets the minimum RSSI threshold display text.
        /// </summary>
        public string MinimumSignalStrengthText => $">= {MinimumSignalStrengthDbm} dBm";

        /// <summary>
        ///     Gets the number of discovered devices.
        /// </summary>
        public int DeviceCount => Devices.Count;

        /// <summary>
        ///     Command to start scanning for BLE devices.
        /// </summary>
        public IAsyncRelayCommand StartScanCommand { get; }

        


        //Roschmi
        public IAsyncRelayCommand RefreshScanStateCommand { get; }

        

        /// <summary>
        ///     Command to stop scanning for BLE devices.
        /// </summary>
        /// 

        public IAsyncRelayCommand StopScanCommand { get; }

        /// <summary>
        ///     Command to select a device from the list.
        /// </summary>
        public IAsyncRelayCommand<IBluetoothRemoteDevice> SelectDeviceCommand { get; }

        /// <summary>
        ///     Command to reset all scanner filters to their defaults.
        /// </summary>
        public IRelayCommand ClearFiltersCommand { get; }

        /// <summary>
        ///     Command to open closest-device scan mode.
        /// </summary>
        public IAsyncRelayCommand OpenClosestDeviceScanCommand { get; }

        /// <summary>
        ///     Starts BLE scanning when the page appears.
        /// </summary>
        public async override ValueTask OnAppearingAsync()
        {
            await base.OnAppearingAsync();
        }

        /// <summary>
        ///     Stops BLE scanning when the page disappears.
        /// </summary>
        public async override ValueTask OnDisappearingAsync()
        {
            await base.OnDisappearingAsync();
        }





        /*****************************************************************************************************/





        #region Region Constructor 

        // public MainPageViewModel(IAppLogger logger, IBluetoothBleService bluetooth)
        public MainPageViewModel(IBluetoothScanner scanner, INavigationService navigation, ILogger<MainPageViewModel> logger)
        {
            _scanner = scanner;
            _navigation = navigation;
            _logger = logger;
             HideUnnamedDevices = true;
             MinimumSignalStrengthDbm = MinRssiValue;
             NamePattern = string.Empty;

            // Initialize commands
             StartScanCommand = new AsyncRelayCommand(StartScanAsync, () => !IsScanning);

             StopScanCommand = new AsyncRelayCommand(StopScanAsync, () => IsScanning);

             RefreshScanStateCommand = new AsyncRelayCommand(RefreshScanStateAsync);


            SelectDeviceCommand = new AsyncRelayCommand<IBluetoothRemoteDevice>(SelectDeviceAsync);

            //   ClearFiltersCommand = new RelayCommand(ClearFilters);

            // OpenClosestDeviceScanCommand = new AsyncRelayCommand(OpenClosestDeviceScanAsync);

            // Subscribe to scanner events
            _scanner.RunningStateChanged += OnRunningStateChanged;
            _scanner.DeviceListChanged += OnDeviceListChanged;



        }

        private async Task RefreshScanStateAsync()
        {
            if (_scanner.IsRunning)
            {
                IsScanning = true;
            }
        }


        private async Task StartScanAsync()
        {

            int breakpoint67 = 0;
           
            if (await _scanner.HasScannerPermissionsAsync())
            {
                try
                {
                    _logger.LogInformation("Starting BLE scan...");

                    var options = new ScanningOptions
                    {
                        IgnoreNamelessAdvertisements = false

                        // Using defaults - scans for all devices
                    };

                    await _scanner.StartScanningAsync();
                    _logger.LogInformation("BLE scan started successfully");


                }
                catch (Exception ex)

                {
                    _logger.LogError(ex, "Failed to start BLE scanning");

                    // Handle scanning errors (permissions, Bluetooth off, etc.)
                    var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (mainPage != null)
                    {
                        await mainPage.DisplayAlertAsync("Scan Error", $"Failed to start scanning: {ex.Message}", "OK");
                    }
                }
 
                int breakpoint57 = 1;


            }
        }


        /// <summary>
        ///     Stops scanning for BLE devices.
        /// </summary>
        private async Task StopScanAsync()
        {
            try
            {
                _logger.LogInformation("Stopping BLE scan...");
                if (_scanner.IsRunning)
                { 
                    await _scanner.StopScanningAsync();
                    _logger.LogInformation("BLE scan stopped successfully");
                }
                else
                {
                    _logger.LogInformation("BLE scan was stopped before");
                }
                
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop BLE scanning");

                var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlertAsync("Stop Error", $"Failed to stop scanning: {ex.Message}", "OK");
                }
            }
        }


        /// <summary>
        ///     Handles device selection and navigates to device details.
        /// </summary>
        /// <param name="device">The selected device.</param>
        private async Task SelectDeviceAsync(IBluetoothRemoteDevice? device)
        {
            if (device == null)
            {
                return;
            }

            _logger.LogInformation("Device selected: {DeviceName} ({DeviceId})", device.Name ?? "Unknown", device.Id);

            SelectedDevice = device;

            int breakpoint323 = 1;


            // Navigate to DevicePage with the selected device
            //await _navigation.NavigateToAsync<DevicePage>(new Dictionary<string, object>
            //{
            //    ["Device"] = device
            //});
        }



        /// <summary>
        ///     Handles changes to the device list (devices added/removed).
        /// </summary>
        private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
        {
            // Update the ObservableCollection on the UI thread
            MainThread.BeginInvokeOnMainThread(() => {
                _allDevices = [.. _scanner.GetDevices()];

              ApplyFilters();

                _logger.LogDebug("Device list updated - Total devices: {DeviceCount}", DeviceCount);
            });
        }

        /// <summary>
        ///     Handles changes to the scanner's running state.
        /// </summary>
        private void OnRunningStateChanged(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                IsScanning = _scanner.IsRunning;
                StartScanCommand.NotifyCanExecuteChanged();
                StopScanCommand.NotifyCanExecuteChanged();
                int breakpoint301 = 1;
            });
        }

        private void ApplyFilters()
        {
            IEnumerable<IBluetoothRemoteDevice> filtered = _allDevices;

            if (HideUnnamedDevices)
            {
                filtered = filtered.Where(device => !string.IsNullOrWhiteSpace(device.Name));
            }

            if (!string.IsNullOrWhiteSpace(NamePattern))
            {
                var pattern = NamePattern.Trim();
                filtered = filtered.Where(device =>
                    !string.IsNullOrWhiteSpace(device.Name) &&
                    device.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase));
            }

            filtered = filtered.Where(device => device.SignalStrengthDbm >= MinimumSignalStrengthDbm);

            Devices.UpdateFrom(filtered.OrderByDescending(device => device.SignalStrengthDbm));
            OnPropertyChanged(nameof(DeviceCount));
        }


        #endregion
    }
}
