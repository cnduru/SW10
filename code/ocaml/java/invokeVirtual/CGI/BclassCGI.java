public class BclassCGI extends AclassCGI{
    public int foo(){ 
        if (VirtualCGI.callId != 30) return -10;
        VirtualCGI.callId = 40;    
        return 3;
    }
}
