﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

using dnlib.DotNet;

using UnispectEx.Core.Inspector;
using UnispectEx.Core.Util;

namespace UnispectEx {
    internal static class Program {
        private static bool ProcessAndSerialize(ImmutableList<MetadataContainer> containers, string outputDirectory) {
            var processor = new DefaultDumpProcessor();
            if (!processor.Initialize(containers))
                return false;

            if (!processor.Serialize(new DefaultDumpSerializer()))
                return false;

            return processor.Write(outputDirectory);
        }

        internal static void Main(string[] args) {
            if (args.Length != 2) {
                Console.WriteLine("[*] UnispectEx <process> <Assembly-CSharp.dll path>");

                return;
            }

            var processName = args[0];
            var dllName = args[1];

            var memory = new LocalMemory();

            if (!memory.Attach(processName))
                return;

            var inspector = new UnityProcess(memory);

            if (!inspector.Initialize("mono-2.0-bdwgc.dll"))
                return;

            var dataDirectory = inspector.DataDirectory();

            if (dataDirectory is null)
                return;

            var assembly = inspector.Domain!.GetAssemblies().FirstOrDefault(x => x.AssemblyName.Name == dllName)!;
            var module = ModuleDefMD.Load(Path.Join(dataDirectory, "Managed", $"{dllName}.dll"));

            var correlatedClasses = Helpers.CorrelateClasses(module.GetTypes(), assembly.Image.Types());
            var containers = correlatedClasses.Select(x => new MetadataContainer(x.Item1, x.Item2)).ToImmutableList();

            var analyzer = new DefaultDumpAnalyzer();

            if (!analyzer.Analyze(containers)) {
                Console.WriteLine("[-] Analysis failed!");

                return;
            }

            Console.WriteLine(ProcessAndSerialize(containers, AppContext.BaseDirectory)
                ? "[*] Successfully dumped and serialized!"
                : "[-] Failed to dump and serialize!");
        }
    }
}