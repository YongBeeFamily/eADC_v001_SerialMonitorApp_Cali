using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace RealTimeGraph
{
    #region PacketStruct
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Gyro_data
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public char label;

        [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
        public Int16 gyro_x;
        [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
        public Int16 gyro_y;
        [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
        public Int16 gyro_z;
        [MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        public float gyro_x_offset;
        [MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        public float gyro_y_offset;
        [MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        public float gyro_z_offset;

    }
    #endregion

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PACKET_REQUEST
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte label;

        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte type;
        public UInt16 status;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PACKET_DEBUG_INFO
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public char label;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.R4)]
        public float[] debug;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PACKET_BOARD_INFO
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte label;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Mainboard;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Firmware;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Coptertype;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ADS2OFP_STRUCT
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header1;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header2;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte CMD_Counter;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte label;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R4)]
        public float[] AirPosVel;       //12
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        public float[] AirData;     //18
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I4)]
        public Int32[] AirData_Raw;     //24
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R4)]
        public float[] AngleData;       //28
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I4)]
        public Int32[] AngleData_Raw;       //32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R4)]
        public float[] PTPStemp;       //28


        [MarshalAs(UnmanagedType.I2, SizeConst = 2)]
        public UInt16 version;     //34
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Checksum;       //35
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GCS2ADS_LOGDATA
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header1;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header2;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Label;                  // 3

        //[MarshalAs(UnmanagedType.R4, SizeConst = 4)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R4)]
        public float[] BasePressure;        // 4 x 2 = 8
        //[MarshalAs(UnmanagedType.U1, SizeConst = 32)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.U1)]
        public byte[] SerialNo;             // 32

        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Checksum;               // 1
    }                                       // = 44



    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ADS2OFP_ADSV3E_STRUCT
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header1;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header2;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte CMD_Counter;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        public float[] AirPosVel;       // 12
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U4)]
        public UInt32[] AirData_Raw;     // 16
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U1)]
        public byte[] ErrorCode;    // 8

        [MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public UInt32 OperationTime;    // 4

        [MarshalAs(UnmanagedType.U2, SizeConst = 1)]
        public UInt16 version;          // 2
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Checksum;           // 1
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ADS2OFP_eADC_STRUCT
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header1;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Header2;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte CMD_Counter;        // 3

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        public float[] AirPosVel;       // 12
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
        public UInt32[] AirData_Raw;    // 8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.R4)]
        public float[] PTPStemp;        // 8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U1)]
        public byte[] bit;              // 3

        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte Checksum;           // 1
    }                                   // 35 = 3 + 12 + 8 + 8 + 1

    //public static class siDataClass
    //{
    //    public static CultureInfo Culture;

    //    //form control
    //    public static bool Form3DIsopened;
    //    public static bool Form3DActivate;

    //    //board Info
    //    public static string Mainboard;

    //    //Debug data
    //    public static float[] debug = new float[16];

    //    //Request!! 
    //    public static UInt16 Board_status;
    //}
    public static class siDataClass
    {
        public static CultureInfo Culture;

        //form control
        public static bool Form3DIsopened;
        public static bool Form3DActivate;

        //board Info
        public static string Mainboard;

        //Debug data
        public static float[] debug = new float[13];

        //Request!! 
        public static UInt16 Board_status;
    }
}
