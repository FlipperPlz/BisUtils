using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Parsers.ParamParser; 

public class ParamBuilder {
    private ParamFile _build = new();

    public ParamBuilder WithEntry(IRapStatement entry) {
        _build.Statements.Add(entry);
        return this;
    }
    
    public ParamBuilder WithExternalClass(string className) {
        _build.Statements.Add(new RapExternalClassStatement(className));
        return this;
    }
    
    public ParamBuilder WithDeleteStatement(string toDelete) {
        _build.Statements.Add(new RapDeleteStatement(toDelete));
        return this;
    }
    
    public ParamBuilder WithArrayAppension(string variableName, ParamArrayBuilder builder) {
        _build.Statements.Add(new RapAppensionStatement(variableName, builder.Build()));
        return this;
    }
    
    public ParamBuilder WithArrayAppension(string variableName, Action<ParamArrayBuilder> builder) {
        var built = new ParamArrayBuilder();
        builder.Invoke(built);
        _build.Statements.Add(new RapAppensionStatement(variableName, built.Build()));
        return this;
    }

    public ParamBuilder WithGlobalVariable(string variableName, string variableValue) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, new RapString(variableValue)));
        return this;
    }
    
    public ParamBuilder WithGlobalVariable(string variableName, int variableValue) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, new RapInteger(variableValue)));
        return this;
    }
    
    public ParamBuilder WithGlobalVariable(string variableName, float variableValue) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, new RapFloat(variableValue)));
        return this;
    }
    
    public ParamBuilder WithGlobalVariable(string variableName, ParamArrayBuilder builder) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, builder.Build()));
        return this;
    }
    
    public ParamBuilder WithGlobalVariable(string variableName, Action<ParamArrayBuilder> builder) {
        var built = new ParamArrayBuilder();
        builder.Invoke(built);
        _build.Statements.Add(new RapVariableDeclaration(variableName, built.Build()));
        return this;
    }
    
    public ParamBuilder WithClass(ParamClassBuilder builder) {
        _build.Statements.Add(builder.Build());
        return this;
    }
    
    public ParamBuilder WithClass(RapClassDeclaration clazz) {
        _build.Statements.Add(clazz);
        return this;
    }
    
    public ParamBuilder WithClass(Action<ParamClassBuilder> builder) {
        var child = new ParamClassBuilder();
        builder.Invoke(child);
        _build.Statements.Add(child.Build());
        return this;
    }
    
    public ParamBuilder WithClass(Action<ParamClassBuilder> builder, string classname, string? parentClass) {
        var child = new ParamClassBuilder(classname, parentClass);
        builder.Invoke(child);
        _build.Statements.Add(child.Build());
        return this;
    }

    public ParamFile Build() => _build;


}


public class ParamArrayBuilder {
    private readonly RapArray _build = new();

    public ParamArrayBuilder WithEntry(string s) {
        _build.Entries.Add(new RapString(s));
        return this;
    }
    
    public ParamArrayBuilder WithEntry(IRapArrayEntry entry) {
        _build.Entries.Add(entry);
        return this;
    }
    
    public ParamArrayBuilder WithEntry(float s) {
        _build.Entries.Add(new RapFloat(s));
        return this;
    }
    
    public ParamArrayBuilder WithEntry(int s) {
        _build.Entries.Add(new RapInteger(s));
        return this;
    }
    
    public ParamArrayBuilder WithEntry(ParamArrayBuilder b) {
        _build.Entries.Add(b.Build());
        return this;
    }
    
    public ParamArrayBuilder WithEntry(Action<ParamArrayBuilder> builder) {
        var child = new ParamArrayBuilder();
        builder.Invoke(child);
        _build.Entries.Add(child.Build());
        return this;
    }

    public RapArray Build() => _build;

}

public class ParamClassBuilder {
    private readonly RapClassDeclaration _build = new();

    public ParamClassBuilder(string classname, string? parent = null) {
        _build.Classname = classname;
        _build.ParentClassname = parent;
    }

    public ParamClassBuilder() {
        
    }

    public ParamClassBuilder WithClassname(string className) {
        _build.Classname = className;
        return this;
    }

    public ParamClassBuilder WithParent(string? parent) {
        _build.ParentClassname = parent;
        return this;
    }

    public ParamClassBuilder WithChildClass(ParamClassBuilder builder) {
        _build.Statements.Add(builder.Build());
        return this;
    }
    
    public ParamClassBuilder WithChildClass(RapClassDeclaration clazz) {
        _build.Statements.Add(clazz);
        return this;
    }
    
    public ParamClassBuilder WithChildClass(Action<ParamClassBuilder> builder) {
        var child = new ParamClassBuilder();
        builder.Invoke(child);
        _build.Statements.Add(child.Build());
        return this;
    }
    
    public ParamClassBuilder WithChildClass(Action<ParamClassBuilder> builder, string classname, string? parentClass) {
        var child = new ParamClassBuilder(classname, parentClass);
        builder.Invoke(child);
        _build.Statements.Add(child.Build());
        return this;
    }

    public ParamClassBuilder WithEntry(IRapStatement entry) {
        _build.Statements.Add(entry);
        return this;
    }

    public ParamClassBuilder WithVariable(string variableName, string variableValue) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, new RapString(variableValue)));
        return this;
    }
    
    public ParamClassBuilder WithVariable(string variableName, int variableValue) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, new RapInteger(variableValue)));
        return this;
    }
    
    public ParamClassBuilder WithVariable(string variableName, float variableValue) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, new RapFloat(variableValue)));
        return this;
    }
    
    public ParamClassBuilder WithVariable(string variableName, ParamArrayBuilder builder) {
        _build.Statements.Add(new RapVariableDeclaration(variableName, builder.Build()));
        return this;
    }
    
    public ParamClassBuilder WithVariable(string variableName, Action<ParamArrayBuilder> builder) {
        var built = new ParamArrayBuilder();
        builder.Invoke(built);
        _build.Statements.Add(new RapVariableDeclaration(variableName, built.Build()));
        return this;
    }
    
    public ParamClassBuilder WithExternalClass(string className) {
        _build.Statements.Add(new RapExternalClassStatement(className));
        return this;
    }
    
    public ParamClassBuilder WithDeleteStatement(string toDelete) {
        _build.Statements.Add(new RapDeleteStatement(toDelete));
        return this;
    }
    
    public ParamClassBuilder WithArrayAppension(string variableName, ParamArrayBuilder builder) {
        _build.Statements.Add(new RapAppensionStatement(variableName, builder.Build()));
        return this;
    }
    
    public ParamClassBuilder WithArrayAppension(string variableName, Action<ParamArrayBuilder> builder) {
        var built = new ParamArrayBuilder();
        builder.Invoke(built);
        _build.Statements.Add(new RapAppensionStatement(variableName, built.Build()));
        return this;
    }

    public RapClassDeclaration Build() => _build;


}