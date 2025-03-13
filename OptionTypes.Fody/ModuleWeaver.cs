using Fody;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OptionTypes.Fody;

/// <summary>
/// 
/// </summary>
public class ModuleWeaver : BaseModuleWeaver
{
    /// <summary>
    /// 
    /// </summary>
    public override void Execute()
    {
        foreach (var type in ModuleDefinition.Types)
        {
            ProcessType(type);
        }
    }

    private void ProcessType(TypeDefinition type)
    {
        foreach (var method in type.Methods)
        {
            if (!method.HasBody)
            {
                continue;
            }

            method.Body.SimplifyMacros();

            var instructions = method.Body.Instructions;
            var processor = method.Body.GetILProcessor();

            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                // this should probably check something more than just the method name...
                if (instruction.OpCode != OpCodes.Callvirt
                    || instruction.Operand is not MethodReference calledMethod
                    || calledMethod.Name != "OrReturn")
                {
                    continue;
                }

                System.Diagnostics.Debugger.Launch();

                var genericResultType = (GenericInstanceType)calledMethod.DeclaringType;
                var resultDefinition = genericResultType.ElementType.Resolve();

                var isErrorGetter = ModuleDefinition.ImportReference(
                    resultDefinition.Methods.First(p => p.Name == "IsErr"));
                var valueGetter = ModuleDefinition.ImportReference(
                    resultDefinition.Methods.Single(p => p.Name == "Unwrap"));

                isErrorGetter = MakeGeneric(isErrorGetter, genericResultType);
                valueGetter = MakeGeneric(valueGetter, genericResultType);

                var tempLocal = new VariableDefinition(genericResultType);
                method.Body.Variables.Add(tempLocal);

                // find or create the result local variable (for the early return)
                var resultLocal = method.Body.Variables.FirstOrDefault(v =>
                    v.VariableType.FullName == genericResultType.FullName &&
                    v != tempLocal);

                if (resultLocal == null)
                {
                    resultLocal = new VariableDefinition(genericResultType);
                    method.Body.Variables.Add(resultLocal);
                }

                // find the end of the try/catch block if in one
                Instruction? endLeave = null;

                // first find the end target of the try block (where the original leave instruction goes to)
                var tryHandler = method.Body.ExceptionHandlers.FirstOrDefault(h =>
                    IsInstructionInRange(instruction, h.TryStart, h.TryEnd));

                if (tryHandler != null)
                {
                    // find the existing leave instruction's target
                    var existingLeave = method.Body.Instructions
                        .FirstOrDefault(x =>
                            (x.OpCode == OpCodes.Leave || x.OpCode == OpCodes.Leave_S) &&
                            x.Offset >= tryHandler.TryStart.Offset &&
                            x.Offset <= tryHandler.TryEnd.Offset);

                    // get the target instruction that the original leave points to
                    endLeave = existingLeave?.Operand as Instruction;
                }

                var afterIf = processor.Create(OpCodes.Nop);

                // Store the result into temp
                processor.InsertBefore(instruction, processor.Create(OpCodes.Stloc, tempLocal));

                // Load temp and check IsError
                processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, tempLocal));
                processor.InsertBefore(instruction, processor.Create(OpCodes.Callvirt, isErrorGetter));
                processor.InsertBefore(instruction, processor.Create(OpCodes.Brfalse, afterIf));

                if (endLeave == null)
                {
                    // we're not in a try catch so we can just return
                    processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, tempLocal));
                    processor.InsertBefore(instruction, processor.Create(OpCodes.Ret));
                }
                else
                {
                    // If error, store temp in result and leave (don't return!)
                    processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, tempLocal));
                    processor.InsertBefore(instruction, processor.Create(OpCodes.Stloc, resultLocal));
                    // Insert custom leave instruction pointing to the existing leave instruction
                    processor.InsertBefore(instruction,
                        processor.Create(OpCodes.Leave, endLeave));
                }

                // after the if get the value from the result
                processor.InsertBefore(instruction, afterIf);
                processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, tempLocal));
                processor.InsertBefore(instruction, processor.Create(OpCodes.Callvirt, valueGetter));

                processor.Remove(instruction);
                i--;
            }

            method.Body.OptimizeMacros();
        }
    }

    private static MethodReference MakeGeneric(MethodReference method, GenericInstanceType genericType)
    {
        var reference = new MethodReference(method.Name, method.ReturnType, genericType)
        {
            HasThis = true,
            ExplicitThis = method.ExplicitThis,
            CallingConvention = method.CallingConvention,
        };

        foreach (var parameter in method.Parameters)
        {
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }

        return reference;
    }

    private static bool IsInstructionInRange(Instruction instruction, Instruction start, Instruction end)
        => instruction.Offset >= start.Offset && instruction.Offset < end.Offset;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
    }
}
