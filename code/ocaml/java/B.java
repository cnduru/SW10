public class B extends A{

    public A a;

    B(){
        a = new A();
    }

    public int foo(){
        return 3;
    }
}
