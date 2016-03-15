public class B extends A{

    public short get_value(){
        reduce();
        return value;
    }

    private void reduce(){
        value /= 2;
    }
}
