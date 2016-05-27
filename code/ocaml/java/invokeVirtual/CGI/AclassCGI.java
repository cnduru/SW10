public class AclassCGI{
    public int foo(){ 
        if (VirtualCGI.callId != 30) return -10;
        VirtualCGI.callId = 40;
        return 1; 
    }
    public int bar(){ 
        if (VirtualCGI.callId != 31) return -10;
        VirtualCGI.callId = 41;     
        return 2; 
    }
}
