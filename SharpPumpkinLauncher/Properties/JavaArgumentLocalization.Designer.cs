﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SharpPumpkinLauncher.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class JavaArgumentLocalization {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal JavaArgumentLocalization() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SharpPumpkinLauncher.Properties.JavaArgumentLocalization", typeof(JavaArgumentLocalization).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enables Java heap optimization. This sets various parameters to be optimal for long-running jobs with intensive memory allocation, based on the configuration of the computer (RAM and CPU). By default, the option is disabled and the heap sizes are configured less aggressively..
        /// </summary>
        internal static string DescriptionAggressiveHeap {
            get {
                return ResourceManager.GetString("DescriptionAggressiveHeap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Requests the VM to touch every page on the Java heap after requesting it from the operating system and before handing memory out to the application. By default, this option is disabled and all pages are committed as the application uses the heap space. .
        /// </summary>
        internal static string DescriptionAlwaysPreTouch {
            get {
                return ResourceManager.GetString("DescriptionAlwaysPreTouch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the number of threads used for concurrent GC. Sets value to approximately 1/4 of the number of parallel garbage collection threads. The default value depends on the number of CPUs available to the JVM..
        /// </summary>
        internal static string DescriptionConcGcThreads {
            get {
                return ResourceManager.GetString("DescriptionConcGcThreads", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enables the option that disables processing of calls to the System.gc() method. This option is disabled by default, meaning that calls to System.gc() are processed. If processing of calls to System.gc() is disabled, then the JVM still performs GC when necessary..
        /// </summary>
        internal static string DescriptionDisableExplicitGc {
            get {
                return ResourceManager.GetString("DescriptionDisableExplicitGc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the size of the regions into which the Java heap is subdivided when using the garbage-first (G1) collector. The value is a power of 2 and can range from 1 MB to 32 MB. The default region size is determined ergonomically based on the heap size with a goal of approximately 2048 regions..
        /// </summary>
        internal static string DescriptionG1HeapRegionSize {
            get {
                return ResourceManager.GetString("DescriptionG1HeapRegionSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the percentage of the heap size to use as the maximum for the young generation size. The default value is 60 percent of your Java heap..
        /// </summary>
        internal static string DescriptionG1MaxNewSizePercent {
            get {
                return ResourceManager.GetString("DescriptionG1MaxNewSizePercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the target number of mixed garbage collections after a marking cycle to collect old regions with at most G1MixedGCLIveThresholdPercent live data. The default is 8 mixed garbage collections. The goal for mixed collections is to be within this target number..
        /// </summary>
        internal static string DescriptionG1MixedGcCountTarget {
            get {
                return ResourceManager.GetString("DescriptionG1MixedGcCountTarget", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the occupancy threshold for an old region to be included in a mixed garbage collection cycle. The default occupancy is 85 percent..
        /// </summary>
        internal static string DescriptionG1MixedGcLiveThresholdPercent {
            get {
                return ResourceManager.GetString("DescriptionG1MixedGcLiveThresholdPercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the percentage of the heap to use as the minimum for the young generation size. The default value is 5 percent of your Java heap..
        /// </summary>
        internal static string DescriptionG1NewSizePercent {
            get {
                return ResourceManager.GetString("DescriptionG1NewSizePercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the percentage of the heap (0 to 50) that&apos;s reserved as a false ceiling to reduce the possibility of promotion failure for the G1 collector. When you increase or decrease the percentage, ensure that you adjust the total Java heap by the same amount. By default, this option is set to 10%..
        /// </summary>
        internal static string DescriptionG1ReservePercent {
            get {
                return ResourceManager.GetString("DescriptionG1ReservePercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Controls adaptive calculation of the old generation occupancy to start background work preparing for an old generation collection. If enabled, G1 uses -XX:InitiatingHeapOccupancyPercent for the first few times as specified by the value of -XX:G1AdaptiveIHOPNumInitialSamples, and after that adaptively calculates a new optimum value for the initiating occupancy automatically. Otherwise, the old generation collection process always starts at the old generation occupancy determined by -XX:InitiatingHeapOccupanc [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DescriptionG1UseAdaptiveIhop {
            get {
                return ResourceManager.GetString("DescriptionG1UseAdaptiveIhop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the percentage of the old generation occupancy (0 to 100) at which to start the first few concurrent marking cycles for the G1 garbage collector. By default, the initiating value is set to 45%. A value of 0 implies nonstop concurrent GC cycles from the beginning until G1 adaptively sets this value..
        /// </summary>
        internal static string DescriptionInitiatingHeapOccupancyPercent {
            get {
                return ResourceManager.GetString("DescriptionInitiatingHeapOccupancyPercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets a target for the maximum GC pause time (in milliseconds). This is a soft goal, and the JVM will make its best effort to achieve it. The specified value doesn&apos;t adapt to your heap size. By default, for G1 the maximum pause time target is 200 milliseconds. The other generational collectors do not use a pause time goal by default..
        /// </summary>
        internal static string DescriptionMaxGcPauseMillis {
            get {
                return ResourceManager.GetString("DescriptionMaxGcPauseMillis", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the number of the stop-the-world (STW) worker threads. The default value depends on the number of CPUs available to the JVM and the garbage collector selected.
        /// </summary>
        internal static string DescriptionParallelGcThreads {
            get {
                return ResourceManager.GetString("DescriptionParallelGcThreads", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enables parallel reference processing.
        /// </summary>
        internal static string DescriptionParallelRefProcEnabled {
            get {
                return ResourceManager.GetString("DescriptionParallelRefProcEnabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Forces JVM to use anonymous memory for Performance Counters instead of a mapped file. This helps to avoid random VM pauses caused by spontaneous disk I/O..
        /// </summary>
        internal static string DescriptionPerfDisableSharedMem {
            get {
                return ResourceManager.GetString("DescriptionPerfDisableSharedMem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unlocks the options that provide experimental features in the JVM. By default, this option is disabled and experimental features aren&apos;t available.
        /// </summary>
        internal static string DescriptionUnlockExperimentalVmOptions {
            get {
                return ResourceManager.GetString("DescriptionUnlockExperimentalVmOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Disables the use of compressed pointers and object references are represented as 32-bit offsets instead of 64-bit pointers, which typically increases performance when running the application with Java heap sizes less than 32 GB. This option works only for 64-bit JVMs..
        /// </summary>
        internal static string DescriptionUseCompressedOops {
            get {
                return ResourceManager.GetString("DescriptionUseCompressedOops", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Garbage-First (G1) garbage collector is a server-style garbage collector, targeted for multiprocessor machines with large memories. It attempts to meet garbage collection (GC) pause time goals with high probability while achieving high throughput. Whole-heap operations, such as global marking, are performed concurrently with the application threads. This prevents interruptions proportional to heap or live-data size..
        /// </summary>
        internal static string DescriptionUseG1Gc {
            get {
                return ResourceManager.GetString("DescriptionUseG1Gc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enables string deduplication. By default, this option is disabled. To use this option, you must enable the garbage-first (G1) garbage collector. String deduplication reduces the memory footprint of String objects on the Java heap by taking advantage of the fact that many String objects are identical. Instead of each String object pointing to its own character array, identical String objects can point to and share the same character array.
        /// </summary>
        internal static string DescriptionUseStringDeduplication {
            get {
                return ResourceManager.GetString("DescriptionUseStringDeduplication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the initial and maximum size (in bytes) of the heap for the young generation (nursery) in the generational collectors. The young generation region of the heap is used for new objects. GC is performed in this region more often than in other regions. If the size for the young generation is too small, then a lot of minor garbage collections are performed. If the size is too large, then only full garbage collections are performed, which can take a long time to complete. It is recommended that you do not se [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DescriptionXmn {
            get {
                return ResourceManager.GetString("DescriptionXmn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sets the minimum and the initial size (in bytes) of the heap. This value must be a multiple of 1024 and greater than 1 MB.
        /// </summary>
        internal static string DescriptionXms {
            get {
                return ResourceManager.GetString("DescriptionXms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies the maximum size (in bytes) of the heap. This value must be a multiple of 1024 and greater than 2 MB.
        /// </summary>
        internal static string DescriptionXmx {
            get {
                return ResourceManager.GetString("DescriptionXmx", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aggressive Heap.
        /// </summary>
        internal static string NameAggressiveHeap {
            get {
                return ResourceManager.GetString("NameAggressiveHeap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aggressive Opts.
        /// </summary>
        internal static string NameAggressiveOpts {
            get {
                return ResourceManager.GetString("NameAggressiveOpts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Always PreTouch.
        /// </summary>
        internal static string NameAlwaysPreTouch {
            get {
                return ResourceManager.GetString("NameAlwaysPreTouch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Conc Gc Threads.
        /// </summary>
        internal static string NameConcGcThreads {
            get {
                return ResourceManager.GetString("NameConcGcThreads", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Disable Explicit GC.
        /// </summary>
        internal static string NameDisableExplicitGc {
            get {
                return ResourceManager.GetString("NameDisableExplicitGc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to G1 Heap Region Size.
        /// </summary>
        internal static string NameG1HeapRegionSize {
            get {
                return ResourceManager.GetString("NameG1HeapRegionSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to G1 Max New Size Percent.
        /// </summary>
        internal static string NameG1MaxNewSizePercent {
            get {
                return ResourceManager.GetString("NameG1MaxNewSizePercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to G1 Mixed Gc Count Target.
        /// </summary>
        internal static string NameG1MixedGcCountTarget {
            get {
                return ResourceManager.GetString("NameG1MixedGcCountTarget", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to G1 Mixed Gc Live Threshold Percent.
        /// </summary>
        internal static string NameG1MixedGcLiveThresholdPercent {
            get {
                return ResourceManager.GetString("NameG1MixedGcLiveThresholdPercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to G1 New Size Percent.
        /// </summary>
        internal static string NameG1NewSizePercent {
            get {
                return ResourceManager.GetString("NameG1NewSizePercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to G1 Reserve Percent.
        /// </summary>
        internal static string NameG1ReservePercent {
            get {
                return ResourceManager.GetString("NameG1ReservePercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to G1 Use Adaptive IHOP.
        /// </summary>
        internal static string NameG1UseAdaptiveIhop {
            get {
                return ResourceManager.GetString("NameG1UseAdaptiveIhop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initiating Heap Occupancy Percent.
        /// </summary>
        internal static string NameInitiatingHeapOccupancyPercent {
            get {
                return ResourceManager.GetString("NameInitiatingHeapOccupancyPercent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Max Gc Pause Millis.
        /// </summary>
        internal static string NameMaxGcPauseMillis {
            get {
                return ResourceManager.GetString("NameMaxGcPauseMillis", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parallel Gc Threads.
        /// </summary>
        internal static string NameParallelGcThreads {
            get {
                return ResourceManager.GetString("NameParallelGcThreads", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parallel RefProc Enabled.
        /// </summary>
        internal static string NameParallelRefProcEnabled {
            get {
                return ResourceManager.GetString("NameParallelRefProcEnabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Perf Disable SharedMem.
        /// </summary>
        internal static string NamePerfDisableSharedMem {
            get {
                return ResourceManager.GetString("NamePerfDisableSharedMem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unlock Experimental Vm Options.
        /// </summary>
        internal static string NameUnlockExperimentalVmOptions {
            get {
                return ResourceManager.GetString("NameUnlockExperimentalVmOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use CompressedOops.
        /// </summary>
        internal static string NameUseCompressedOops {
            get {
                return ResourceManager.GetString("NameUseCompressedOops", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use G1GC.
        /// </summary>
        internal static string NameUseG1Gc {
            get {
                return ResourceManager.GetString("NameUseG1Gc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use PerfData.
        /// </summary>
        internal static string NameUsePerfData {
            get {
                return ResourceManager.GetString("NameUsePerfData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use String Deduplication.
        /// </summary>
        internal static string NameUseStringDeduplication {
            get {
                return ResourceManager.GetString("NameUseStringDeduplication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Minimum heap size (Xmn).
        /// </summary>
        internal static string NameXmn {
            get {
                return ResourceManager.GetString("NameXmn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Maximum GC memory size (Xms).
        /// </summary>
        internal static string NameXms {
            get {
                return ResourceManager.GetString("NameXms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Maximum heap size (Xmx).
        /// </summary>
        internal static string NameXmx {
            get {
                return ResourceManager.GetString("NameXmx", resourceCulture);
            }
        }
    }
}