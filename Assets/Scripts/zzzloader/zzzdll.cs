using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class zzzdll
{
    const string zzz = @"bot.dll";

    [DllImport(zzz, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    //[return: MarshalAs(UnmanagedType.LPStr)]
    public static extern IntPtr AIName(int Level);


    [DllImport(zzz, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public static extern int AIDllVersion();


    [DllImport(zzz, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr TetrisAI(int[] overfield, int[] field, int field_w, int field_h, int b2b, int combo,
           [In][MarshalAs(UnmanagedType.LPStr,SizeConst = 8)] string next, char hold, bool curCanHold, char active, int x, int y, int spin,
           bool canhold, bool can180spin, int upcomeAtt, int[] comboTable, int maxDepth, int level, int player);
}
