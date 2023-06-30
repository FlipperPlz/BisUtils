namespace BisUtils.PreProcessor.RV.Utils;

using System.Text;
using FResults;
using FResults.Reasoning;
using Models.Directives;

public delegate IEnumerable<IReason> RVIncludeFinder(IRVIncludeDirective include, ref string includeContents);

