public class B extends A{

    public short bar(){
        reduce();
        return value;
    }

    private void reduce(){
        value /= 2;
    }
}
