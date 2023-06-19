namespace BisUtils.PreProcessor.RV.Utils;

using FResults;
using Models.Directives;

public delegate Result RVIncludeFinder(IRVIncludeDirective include, out string fileText);

