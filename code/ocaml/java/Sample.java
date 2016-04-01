public class Sample{
    public A a;
    public A b;

    public static void main(String[] args) {
        Sample s = new Sample();
    }

    Sample(){
        install();
    }
    //may add paramenters later
    private void install(){
        a = new A();
        b = new B();
    }
}
