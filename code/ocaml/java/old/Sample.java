public class Sample{
    public static void main(String[] args) {
        A test = args[0] == "a" ? new A() : new B();
        for(short i=0; i<3; i++){
              test.foo(i);
              test.bar();
        }
        foo(true, false);
    }

    public static void foo(boolean b, boolean b2){
        (b != b2 ? 
            new A() 
          : new C()
        ).bar();
    }
}
