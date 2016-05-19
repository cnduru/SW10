using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRewriter
{
    public static class XMLProvider
    {
        public static string getFaultTemplate()
        {
            return @"<template>
		<name>FaultInj</name>
		<location id=""id0"" x=""-17"" y=""-85"">
             <label kind=""exponentialrate"" x=""-205"" y=""17"">1</label>
		</location>
		<location id=""id1"" x=""-144"" y=""-85"">
             <committed/>
		</location>
		<init ref=""id1""/>
		<transition>
			<source ref=""id1""/>
			<target ref=""id0""/>
			<label kind=""select"" x=""-110"" y=""-127"">i:int[0,80]</label>
			<label kind=""assignment"" x=""-110"" y=""-68"">faultAt = i</label>
		</transition>
	</template>";
        }

        public static string getHeapFaultTemplate()
        {
            return @"	<template>
		<name>heapFault</name>
		<location id=""id3"" x=""-195"" y=""-17"">
            <label kind=""exponentialrate"" x=""-205"" y=""17"">1</label>
		</location>
		<location id=""id4"" x=""-586"" y=""-17"">
			<committed/>
		</location>
		<init ref=""id4""/>
		<transition>
			<source ref=""id4""/>
			<target ref=""id3""/>
			<label kind=""select"" x=""-493"" y=""-42"">i:int[0,80], ii:int[0,7]</label>
			<label kind=""assignment"" x=""-493"" y=""-17"">faultTime = i, bitPosHeap = ii</label>
		</transition>
	</template>";
        }
        public static string getFaultInjTemplate()
        {
            return @"<template>
		<name>FaultInj</name>
		<location id=""id0"" x=""-17"" y=""-85"">
            <label kind=""exponentialrate"" x=""-205"" y=""17"">1</label>
		</location>
		<location id=""id1"" x=""-144"" y=""-85"">
            <committed/>
		</location>
		<init ref=""id1""/>
		<transition>
			<source ref=""id1""/>
			<target ref=""id0""/>
			<label kind=""assignment"" x=""-110"" y=""-68"">faultTime = i</label>
			<label kind=""select"" x=""-100"" y=""-58"">i:int[0,80]</label>
		</transition>
	</template>";
        }

        public static string getInstructionFaultTemplate()
        {
            return @"<template>
		<name>InstFaultInj</name>
		<location id=""id0"" x=""-17"" y=""-85"">
		</location>
		<location id=""id1"" x=""-144"" y=""-85"">
		</location>
		<init ref=""id1""/>
		<transition>
			<source ref=""id1""/>
			<target ref=""id0""/>
			<label kind=""assignment"" x=""-110"" y=""-68"">faultAtId = i</label>
		</transition>
	</template>";
        }
    }
}
