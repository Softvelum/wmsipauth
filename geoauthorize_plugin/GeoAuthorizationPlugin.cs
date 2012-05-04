using System;
using Microsoft.WindowsMediaServices.Interop;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Collections.Generic;
using wmspanel_plugin;
namespace WMSPanel
{
    [ComVisible(true)]
    [Guid("143EE37D-DD91-4130-B9a7-34393B7574E2")]
    public class GeoAuthorizationPlugin : IWMSBasicPlugin, IWMSEventAuthorizationPlugin	{
        private IPList denyList = null;
        private DateTime lastWriteTime = DateTime.Now;
        private static string system32dir = System.IO.Directory.GetCurrentDirectory();
        private static string denylistname = system32dir + "\\windows media\\server\\geodeny.list";

        public GeoAuthorizationPlugin()
		{
            
		}
		#region IWMSBasicPlugin Members

		public void OnHeartbeat()
		{
		}

		public void DisablePlugin()
		{
		}

		public object GetCustomAdminInterface()
		{
            return null;
		}

		public void EnablePlugin(ref int plFlags, ref int plHeartbeatPeriod)
		{
		}

		public void ShutdownPlugin()
		{
		}

		public void InitializePlugin(IWMSContext pServerContext, WMSNamedValues pNamedValues, IWMSClassObject pClassFactory)
		{
		}

		#endregion

		#region IWMSEventAuthorizationPlugin members 

		public void AuthorizeEvent(ref WMS_EVENT pEvent, IWMSContext pUserCtx, IWMSContext pPresentationCtx, IWMSCommandContext pCommandCtx,
							IWMSEventAuthorizationCallback pCallback,
							object Context)
		{
			int hr = 0; // By deafault access is granted to user
			const int ACCESS_DENIED = unchecked((int)0x80070005);
			
			string user_ip_address = null;
			pUserCtx.GetStringValue(WMSDefines.WMS_USER_IP_ADDRESS_STRING,
									WMSDefines.WMS_USER_IP_ADDRESS_STRING_ID,
									out user_ip_address,
				                    0);


            try
            {
                DateTime lastWrite = System.IO.File.GetLastWriteTime(denylistname);

                if (denyList == null)
                {
                    denyList = IPListLoader.loadIpList(denylistname);
                    lastWriteTime = lastWrite;
                }
                else if (lastWriteTime.Ticks != lastWrite.Ticks)
                {
                    denyList = IPListLoader.loadIpList(denylistname);
                    lastWriteTime = lastWrite;
                }
                
                if (denyList.CheckNumber(user_ip_address))
                {
                    hr = ACCESS_DENIED;
                }
            }
            catch (Exception e) { }

			pCallback.OnAuthorizeEvent(hr, Context);			
		}

		public object GetAuthorizedEvents()
		{
				// Identify the events the plug-in can authorize.
				WMS_EVENT_TYPE[] wmsEvents = {WMS_EVENT_TYPE.WMS_EVENT_OPEN};
				return (object)wmsEvents;
		}
		
		#endregion

		[ComRegisterFunctionAttribute]
		public static void RegisterFunction(Type t)
		{


			try
			{
				RegistryKey regHKLM = Registry.LocalMachine;
		        regHKLM = regHKLM.CreateSubKey("SOFTWARE\\Microsoft\\Windows Media\\Server\\RegisteredPlugins\\Event Notification and Authorization\\{143EE37D-DD91-4130-B9a7-34393B7574E2}");
				regHKLM.SetValue(null, "Authorization geo-based plugin"); 

				RegistryKey regHKCR = Registry.ClassesRoot;
                		regHKCR = regHKCR.CreateSubKey("CLSID\\{143EE37D-DD91-4130-B9a7-34393B7574E2}\\Properties");
                		regHKCR.SetValue("Name", "Authorization Geo based plugin");
				regHKCR.SetValue("Author", "Polox Group");
				regHKCR.SetValue("Copyright", "Copyright 2011. All rights reserved");
				regHKCR.SetValue("Description", "Enable to protect video contents based on user geo location");
				regHKCR.SetValue("SubCategory", "Authorize");
			}
			catch(Exception e)
			{
				// too strange 
			}
		}
		[ComUnregisterFunctionAttribute]
		public static void UnRegisterFunction(Type t)
		{
			try
			{
				RegistryKey regHKLM = Registry.LocalMachine;
		                regHKLM.DeleteSubKey("SOFTWARE\\Microsoft\\Windows Media\\Server\\RegisteredPlugins\\Event Notification and Authorization\\{143EE37D-DD91-4130-B9a7-34393B7574E2}");

				RegistryKey regHKCR = Registry.ClassesRoot;
                		regHKCR.DeleteSubKeyTree("CLSID\\{143EE37D-DD91-4130-B9a7-34393B7574E2}");
                		regHKCR.DeleteSubKeyTree("WMSPanel.GeoAuthorizationPlugin");
			}
			catch(Exception e)
			{
			}
		}

    }
}