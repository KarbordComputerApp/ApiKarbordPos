using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Newtonsoft.Json;

namespace ApiKarbordPos.Controllers
{
    public class Web_DataController : ApiController
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        static extern bool FreeLibrary(int hModule);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        delegate bool GetVer(StringBuilder RetVal);



        public static string CallGetVer()
        {
            string dllPath = HttpContext.Current.Server.MapPath("~/Content/Dll/PosBank.dll");
            const string functionName = "GetVer";

            int libHandle = LoadLibrary(dllPath);
            if (libHandle == 0)
                return string.Format("Could not load library \"{0}\"", dllPath);
            try
            {
                var delphiFunctionAddress = GetProcAddress(libHandle, functionName);
                if (delphiFunctionAddress == IntPtr.Zero)
                    return string.Format("Can't find function \"{0}\" in library \"{1}\"", functionName, dllPath);

                var delphiFunction = (GetVer)Marshal.GetDelegateForFunctionPointer(delphiFunctionAddress, typeof(GetVer));

                StringBuilder RetVal = new StringBuilder(1024);
                delphiFunction(RetVal);
                return RetVal.ToString();
            }
            finally
            {
                FreeLibrary(libHandle);
            }
        }


        [Route("api/Web_Data/GetVer")]
        public async Task<IHttpActionResult> Get_GetVer()
        {
            string log = "";
            log = CallGetVer();

            return Ok(log);
        }





        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        delegate bool Pay_PosBan(int BaudRate, int port, int PosBanMode, string Value, string Factor_NO, string Prn_Message, StringBuilder RetVal);

        public static string CallPay_PosBan(int BaudRate, int port, int PosBanMode, string Value, string Factor_NO, string Prn_Message)
        {
            string dllName = "PosBank.dll";
            string dllPath = HttpContext.Current.Server.MapPath("~/Content/Dll/" + dllName);
            const string functionName = "Pay_PosBan";

            int libHandle = LoadLibrary(dllPath);
            if (libHandle == 0)
                return string.Format("Could not load library \"{0}\"", dllPath);
            try
            {
                var delphiFunctionAddress = GetProcAddress(libHandle, functionName);
                if (delphiFunctionAddress == IntPtr.Zero)
                    return string.Format("Can't find function \"{0}\" in library \"{1}\"", functionName, dllPath);

                var delphiFunction = (Pay_PosBan)Marshal.GetDelegateForFunctionPointer(delphiFunctionAddress, typeof(Pay_PosBan));

                StringBuilder RetVal = new StringBuilder(1024);
                delphiFunction(
                    BaudRate,
                    port,
                    PosBanMode,
                    Value,
                    Factor_NO,
                    Prn_Message,
                    RetVal);
                return RetVal.ToString();
            }
            finally
            {
                FreeLibrary(libHandle);
            }
        }




        public class Pay_PosBanObject
        {
            public int BaudRate { get; set; }
            public int Port { get; set; }
            public int PosBanMode { get; set; }
            public string Factor_NO { get; set; }
            public string Value { get; set; }
            public string Prn_Message { get; set; }

        }


        [Route("api/Web_Data/Pay_PosBan")]
        public async Task<IHttpActionResult> PostPay_PosBan(Pay_PosBanObject Pay_PosBanObject)
        {
            string log = "";
            log = CallPay_PosBan(
                   Pay_PosBanObject.BaudRate,
                   Pay_PosBanObject.Port,
                   Pay_PosBanObject.PosBanMode,
                   Pay_PosBanObject.Value,
                   Pay_PosBanObject.Factor_NO,
                   Pay_PosBanObject.Prn_Message
                );

            return Ok(log);
        }














        public class PosListObject
        {
            public string Code { get; set; }

            public string Name { get; set; }

            public string AccCode { get; set; }

            public string ModePos { get; set; }

            public string ComPorts { get; set; }

            public string BuadRates { get; set; }

            public string Parity { get; set; }

            public string DataBit { get; set; }

            public string Time { get; set; }

            public string SerialNo { get; set; }
        }




        [Route("api/Web_Data/PosList")]
        public async Task<IHttpActionResult> GetWeb_PosList()
        {

            RegistryKey OurKey = Registry.CurrentUser;
            OurKey = OurKey.OpenSubKey(@"Software\Karbord\Pos\", true);

            

            List<PosListObject> list = new List<PosListObject>();

            foreach (string Keyname in OurKey.GetSubKeyNames())
            {
                RegistryKey key = OurKey.OpenSubKey(Keyname);
                string a = key.GetValue("AccCode").ToString();


                PosListObject item = new PosListObject
                {
                    Code = Keyname,
                    AccCode = key.GetValue("AccCode").ToString(),
                    BuadRates = key.GetValue("BuadRates").ToString(),
                    ComPorts = key.GetValue("ComPorts").ToString(),
                    DataBit = key.GetValue("DataBit").ToString(),
                    ModePos = key.GetValue("ModePos").ToString(),
                    Name = key.GetValue("Name").ToString(),
                    Parity = key.GetValue("Parity").ToString(),
                    SerialNo = key.GetValue("SerialNo").ToString(),
                    Time = key.GetValue("Time").ToString()
                };

                list.Add(item);
            }
            return Ok(list);

        }








    }
}
