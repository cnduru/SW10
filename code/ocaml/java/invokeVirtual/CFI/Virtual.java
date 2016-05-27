public class Virtual{
  public Aclass a;
  public Aclass b;

  public static int flag = 0;

  public static void main(String[] args) 
  {
      try{
          install();
      }catch (Exception ex){
      }
      
  }

  public static void install() throws Exception
  {
      new Virtual();

      if(flag != 3){
          throw new Exception();
      }
  }

  public Virtual() throws Exception
  { 
      a = new Aclass();
      b = new Bclass();

      int ia = a.foo() + a.bar();
      int ib = b.foo() + b.bar();
  }
}
