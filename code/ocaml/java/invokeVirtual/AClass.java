public class AClass{
  public static void main(String[] args) {
        AClass a = new AClass();
        AClass b = new BClass();

        int ia = a.foo() + a.bar();
        int ib = b.foo() + b.bar();
    }

  public int foo(){ return 1; }
  public int bar(){ return 2; }
}
