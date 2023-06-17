﻿namespace BisUtils.Param.Options;

using System.Text;
using Core.Binarize.Options;
using Core.Binarize.Utils;
using Core.Options;

public class ParamOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public int AsciiLengthTimeout { get; set; } = 1024;
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Little;
    public bool IgnoreValidation { get; set; }
}