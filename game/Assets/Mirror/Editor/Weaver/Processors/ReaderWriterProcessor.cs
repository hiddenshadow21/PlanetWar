// finds all readers and writers and register them
using System;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Mirror.Weaver
{
    public static class ReaderWriterProcessor
    {
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
        public static bool Process(AssemblyDefinition CurrentAssembly)
        {
            Readers.Init();
            Writers.Init();
<<<<<<< HEAD
=======
        public static void Process(AssemblyDefinition CurrentAssembly)
        {
            Readers.Init();
            Writers.Init();

>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
            foreach (Assembly unityAsm in CompilationPipeline.GetAssemblies())
            {
                if (unityAsm.name == "Mirror")
                {
                    using (DefaultAssemblyResolver asmResolver = new DefaultAssemblyResolver())
                    using (AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(unityAsm.outputPath, new ReaderParameters { ReadWrite = false, ReadSymbols = false, AssemblyResolver = asmResolver }))
                    {
                        ProcessAssemblyClasses(CurrentAssembly, assembly);
                    }
                }
            }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
            return ProcessAssemblyClasses(CurrentAssembly, CurrentAssembly);
        }

        static bool ProcessAssemblyClasses(AssemblyDefinition CurrentAssembly, AssemblyDefinition assembly)
        {
            bool modified = false;
<<<<<<< HEAD
=======
            ProcessAssemblyClasses(CurrentAssembly, CurrentAssembly);
        }

        static void ProcessAssemblyClasses(AssemblyDefinition CurrentAssembly, AssemblyDefinition assembly)
        {
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
            foreach (TypeDefinition klass in assembly.MainModule.Types)
            {
                // extension methods only live in static classes
                // static classes are represented as sealed and abstract
                if (klass.IsAbstract && klass.IsSealed)
                {
<<<<<<< HEAD
<<<<<<< HEAD
                    // if asmebly has any declared writers then it is "modified"
                    modified |= LoadDeclaredWriters(CurrentAssembly, klass);
                    modified |= LoadDeclaredReaders(CurrentAssembly, klass);
=======
                    LoadDeclaredWriters(CurrentAssembly, klass);
                    LoadDeclaredReaders(CurrentAssembly, klass);
>>>>>>> feature/MenuEsc
=======
                    // if asmebly has any declared writers then it is "modified"
                    modified |= LoadDeclaredWriters(CurrentAssembly, klass);
                    modified |= LoadDeclaredReaders(CurrentAssembly, klass);
>>>>>>> feature/MainMenu
                }
            }

            foreach (TypeDefinition klass in assembly.MainModule.Types)
            {
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
                // if asmebly has any network message then it is modified
                modified |= LoadMessageReadWriter(CurrentAssembly.MainModule, klass);
            }
            return modified;
        }

        static bool LoadMessageReadWriter(ModuleDefinition module, TypeDefinition klass)
        {
            bool modified = false;
<<<<<<< HEAD
=======
                LoadMessageReadWriter(CurrentAssembly.MainModule, klass);
            }
        }

        private static void LoadMessageReadWriter(ModuleDefinition module, TypeDefinition klass)
        {
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
            if (!klass.IsAbstract && !klass.IsInterface && klass.ImplementsInterface<NetworkMessage>())
            {
                Readers.GetReadFunc(module.ImportReference(klass));
                Writers.GetWriteFunc(module.ImportReference(klass));
<<<<<<< HEAD
<<<<<<< HEAD
                modified = true;
=======
>>>>>>> feature/MenuEsc
=======
                modified = true;
>>>>>>> feature/MainMenu
            }

            foreach (TypeDefinition td in klass.NestedTypes)
            {
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
                modified |= LoadMessageReadWriter(module, td);
            }
            return modified;
        }

        static bool LoadDeclaredWriters(AssemblyDefinition currentAssembly, TypeDefinition klass)
        {
            // register all the writers in this class.  Skip the ones with wrong signature
            bool modified = false;
<<<<<<< HEAD
=======
                LoadMessageReadWriter(module, td);
            }
        }

        static void LoadDeclaredWriters(AssemblyDefinition currentAssembly, TypeDefinition klass)
        {
            // register all the writers in this class.  Skip the ones with wrong signature
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
            foreach (MethodDefinition method in klass.Methods)
            {
                if (method.Parameters.Count != 2)
                    continue;

                if (!method.Parameters[0].ParameterType.Is<NetworkWriter>())
                    continue;

                if (!method.ReturnType.Is(typeof(void)))
                    continue;

                if (!method.HasCustomAttribute<System.Runtime.CompilerServices.ExtensionAttribute>())
                    continue;

                if (method.HasGenericParameters)
                    continue;

                TypeReference dataType = method.Parameters[1].ParameterType;
                Writers.Register(dataType, currentAssembly.MainModule.ImportReference(method));
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
                modified = true;
            }
            return modified;
        }

        static bool LoadDeclaredReaders(AssemblyDefinition currentAssembly, TypeDefinition klass)
        {
            // register all the reader in this class.  Skip the ones with wrong signature
            bool modified = false;
<<<<<<< HEAD
=======
            }
        }

        static void LoadDeclaredReaders(AssemblyDefinition currentAssembly, TypeDefinition klass)
        {
            // register all the reader in this class.  Skip the ones with wrong signature
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
            foreach (MethodDefinition method in klass.Methods)
            {
                if (method.Parameters.Count != 1)
                    continue;

                if (!method.Parameters[0].ParameterType.Is<NetworkReader>())
                    continue;

                if (method.ReturnType.Is(typeof(void)))
                    continue;

                if (!method.HasCustomAttribute<System.Runtime.CompilerServices.ExtensionAttribute>())
                    continue;

                if (method.HasGenericParameters)
                    continue;

                Readers.Register(method.ReturnType, currentAssembly.MainModule.ImportReference(method));
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
                modified = true;
            }
            return modified;
        }

        static bool IsEditorAssembly(AssemblyDefinition currentAssembly)
        {
            return currentAssembly.MainModule.AssemblyReferences.Any(assemblyReference =>
                assemblyReference.Name == nameof(UnityEditor)
                );
<<<<<<< HEAD
=======
            }
        }

        private static bool IsEditorAssembly(AssemblyDefinition currentAssembly)
        {
            return currentAssembly.MainModule.AssemblyReferences.Any(assemblyReference =>
                assemblyReference.Name == nameof(UnityEditor)
                ) ;
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
        }

        /// <summary>
        /// Creates a method that will store all the readers and writers into
        /// <see cref="Writer{T}.write"/> and <see cref="Reader{T}.read"/>
        ///
        /// The method will be marked InitializeOnLoadMethodAttribute so it gets
        /// executed before mirror runtime code
        /// </summary>
        /// <param name="currentAssembly"></param>
        public static void InitializeReaderAndWriters(AssemblyDefinition currentAssembly)
        {
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> feature/MainMenu
            MethodDefinition rwInitializer = new MethodDefinition("InitReadWriters", MethodAttributes.Public |
                    MethodAttributes.Static,
                    WeaverTypes.Import(typeof(void)));

            System.Reflection.ConstructorInfo attributeconstructor = typeof(RuntimeInitializeOnLoadMethodAttribute).GetConstructor(new[] { typeof(RuntimeInitializeLoadType) });
<<<<<<< HEAD
=======
            var rwInitializer = new MethodDefinition("InitReadWriters", MethodAttributes.Public |
                    MethodAttributes.Static,
                    WeaverTypes.Import(typeof(void)));

            System.Reflection.ConstructorInfo attributeconstructor = typeof(RuntimeInitializeOnLoadMethodAttribute).GetConstructor(new [] { typeof(RuntimeInitializeLoadType)});
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu

            CustomAttribute customAttributeRef = new CustomAttribute(currentAssembly.MainModule.ImportReference(attributeconstructor));
            customAttributeRef.ConstructorArguments.Add(new CustomAttributeArgument(WeaverTypes.Import<RuntimeInitializeLoadType>(), RuntimeInitializeLoadType.BeforeSceneLoad));
            rwInitializer.CustomAttributes.Add(customAttributeRef);

            if (IsEditorAssembly(currentAssembly))
            {
                // editor assembly,  add InitializeOnLoadMethod too.  Useful for the editor tests
                System.Reflection.ConstructorInfo initializeOnLoadConstructor = typeof(InitializeOnLoadMethodAttribute).GetConstructor(new Type[0]);
                CustomAttribute initializeCustomConstructorRef = new CustomAttribute(currentAssembly.MainModule.ImportReference(initializeOnLoadConstructor));
                rwInitializer.CustomAttributes.Add(initializeCustomConstructorRef);
            }

            ILProcessor worker = rwInitializer.Body.GetILProcessor();

            Writers.InitializeWriters(worker);
            Readers.InitializeReaders(worker);

            worker.Append(worker.Create(OpCodes.Ret));

<<<<<<< HEAD
<<<<<<< HEAD
=======
            Weaver.WeaveLists.ConfirmGeneratedCodeClass();
>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
            TypeDefinition generateClass = Weaver.WeaveLists.generateContainerClass;

            generateClass.Methods.Add(rwInitializer);
        }
<<<<<<< HEAD
<<<<<<< HEAD
=======

>>>>>>> feature/MenuEsc
=======
>>>>>>> feature/MainMenu
    }
}
