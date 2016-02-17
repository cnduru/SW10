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
			<committed/>
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
    }
}
