﻿using System.Collections.ObjectModel;

namespace BisUtils.Core.Family;

public interface IFamilyNode : IFamilyParent
{
    IFamilyNode? IFamilyMember.Node =>  this;
    
}