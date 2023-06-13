﻿using BisUtils.Core.Binarize.Options;
using BisUtils.Core.IO;

namespace BisUtils.Core.Binarize.Implementation;

public abstract class StrictBinaryObject<T> : BinaryObject<T>, IStrictBinaryObject<T> where T : IBinarizationOptions
{
    protected StrictBinaryObject(BisBinaryReader reader, T options) : base(reader, options)
    {
    }

    protected StrictBinaryObject()
    {
    }

    public abstract bool Validate(T options);
}