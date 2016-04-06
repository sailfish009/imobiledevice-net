﻿using iMobileDevice;
using iMobileDevice.iDevice;
using iMobileDevice.Lockdown;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileDevice.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // This demo application will use the libimobiledevice API to list all iOS devices currently
            // connected to this PC.

            // First, we need to make sure the unmanaged (native) libimobiledevice libraries are loaded correctly
            NativeLibraries.Load();

            ReadOnlyCollection<string> udids;
            int count = 0;

            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;

            var ret = idevice.idevice_get_device_list(out udids, ref count);

            if (ret == iDeviceError.NoDevice)
            {
                // Not actually an error in our case
                return;
            }

            ret.ThrowOnError();

            // Get the device name
            foreach (var udid in udids)
            {
                iDeviceHandle deviceHandle;
                idevice.idevice_new(out deviceHandle, udid).ThrowOnError();

                LockdownClientHandle lockdownHandle;
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "Quamotion").ThrowOnError();

                IntPtr deviceNamePtr = IntPtr.Zero;
                lockdown.lockdownd_get_device_name(lockdownHandle, ref deviceNamePtr).ThrowOnError();

                var deviceName = Marshal.PtrToStringAnsi(deviceNamePtr);
                deviceHandle.Dispose();
                lockdownHandle.Dispose();
            }
        }
    }
}
