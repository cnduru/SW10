public class Sample{
    public A a;
    public A b;

    public static void main(String[] args) {
        Sample s = new Sample();
        foo(true, false);
    }

    Sample(){
        install();
    }
    //may add paramenters later
    private void install(){
        a = new A();
        b = new B();
    }

    public static int foo(boolean b, boolean b2){
        return b != b2 ? 1 : 2;
        
    }
}
