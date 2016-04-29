public class Virtual{
  public Aclass a;
  public Aclass b;

  public static void main(String[] args) 
  {
    install();
  }

  public static void install()
  {
    new Virtual();
  }

  public Virtual()
  {
    a = new Aclass();
    b = new Bclass();
    int ia = a.foo() + a.bar();
    int ib = b.foo() + b.bar();
  }
}
