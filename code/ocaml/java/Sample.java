public class Sample{
    public static void main(String[] args) {
        A test = args[0] == "a" ? new A() : new B();
        for(short i=0; i<3; i++){
              test.foo(i);
              test.bar();
        }
    }

    public void foo(boolean b){
        (b ? new A() : new B()).bar();
    }
}
