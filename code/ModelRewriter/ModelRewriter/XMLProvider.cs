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
		</location>
		<location id=""id1"" x=""-144"" y=""-85"">
		</location>
		<init ref=""id1""/>
		<transition>
			<source ref=""id1""/>
			<target ref=""id0""/>
			<label kind=""select"" x=""-110"" y=""-127"">i:int[0,7]</label>
			<label kind=""assignment"" x=""-110"" y=""-68"">faultAt = i</label>
		</transition>
	</template>";
        }

        public static string getDataFaultTemplate()
        {
            return @"	<template>
		<name>dataFault</name>
		<location id=""id2"" x=""25"" y=""-127"">
		</location>
		<location id=""id3"" x=""-195"" y=""-17"">
		</location>
		<location id=""id4"" x=""-586"" y=""-17"">
			<committed/>
		</location>
		<init ref=""id4""/>
		<transition>
			<source ref=""id3""/>
			<target ref=""id2""/>
			<label kind=""select"" x=""51"" y=""-153"">heapIndex:int[0,heap_size]</label>
			<label kind=""guard"" x=""-177"" y=""-106"">faultClock == faultTime</label>
			<label kind=""synchronisation"" x=""-177"" y=""-89"">f?</label>
			<label kind=""assignment"" x=""51"" y=""-127"">H[heapIndex] ^= 1 &lt;&lt; bitPos, faultClock = 0</label>
		</transition>
		<transition>
			<source ref=""id4""/>
			<target ref=""id3""/>
			<label kind=""select"" x=""-493"" y=""-42"">i:int[0,46], ii:int[0,7]</label>
			<label kind=""assignment"" x=""-493"" y=""-17"">faultTime = i, bitPos = ii</label>
		</transition>
	</template>";
        }
    }
}
