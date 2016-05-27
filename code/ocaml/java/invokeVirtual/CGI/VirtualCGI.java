public class VirtualCGI{
  public AclassCGI a;
  public AclassCGI b;
  public static int callId;

  public static void main(String[] args) 
  {
    install();
  }

  public static void install()
  {
    new VirtualCGI();
  }

  public VirtualCGI()
  {
    a = new AclassCGI();
    b = new BclassCGI();
    VirtualCGI.callId = 30;
    int ia = a.foo() + a.bar();
    if (VirtualCGI.callId != 40) ia = -10;
    VirtualCGI.callId = 31;
    int ib = b.foo() + b.bar();
    if (VirtualCGI.callId != 40) ib = -10;
  }
}
