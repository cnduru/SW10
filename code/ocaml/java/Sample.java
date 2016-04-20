public class Sample{
    public A a;
    public A b;

    public static void main(String[] args) {
        install();
        foo(3);
    }

    Sample(){
       a = new A();
       b = new B();
    }
    //may add paramenters later
    private static void install(){
      new Sample();
    }

    public static int foo(int i){
        return i != 0 ? 1 : 2;
    }
}
