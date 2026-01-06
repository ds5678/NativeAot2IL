using System.Reflection;

namespace NativeAot2IL.Metadata;

public class MethodDefinition : ReadableClass
{ 
    public MethodAttributes Flags { get; private set; }
    public MethodImplAttributes ImplFlags { get; private set; }
    public string? Name { get; private set; }
    private MetadataHandle _nameHandle;  //ConstantStringValue
    public MetadataHandle Signature { get; private set; } //MethodSignature
    public MetadataHandle[] Parameters { get; private set; } //Parameter
    public MetadataHandle[] GenericParameters { get; private set; } //GenericParameter
    public MetadataHandle[] CustomAttributes { get; private set; } //CustomAttribute
    
    public override void Read(ClassReadingBinaryReader reader)
    {
        Flags = (MethodAttributes)reader.ReadCompressedUIntHereNoLock();
        ImplFlags = (MethodImplAttributes)reader.ReadCompressedUIntHereNoLock();
        _nameHandle = reader.ReadMetadataHandleHereNoLock(HandleType.ConstantStringValue);
        Signature = reader.ReadMetadataHandleHereNoLock(HandleType.MethodSignature);
        Parameters = reader.ReadMetadataHandleArrayHereNoLock(HandleType.Parameter);
        GenericParameters = reader.ReadMetadataHandleArrayHereNoLock(HandleType.GenericParameter);
        CustomAttributes = reader.ReadMetadataHandleArrayHereNoLock(HandleType.CustomAttribute);

        Name = _nameHandle.ResolveString(reader, false);
    }
    
    public override string ToString()
    {
        return $"MethodDefinition: {Name}, Parameters={Parameters.Length}, GenericParameters={GenericParameters.Length}, CustomAttributes={CustomAttributes.Length}, ImplFlags={ImplFlags}, Flags={Flags}";
    }
}