public class B extends A{    
    public short get_value(){
        reduce();
        return value;
    }

    public short add(short v){
        value += v/2;        
        return value;
    }

    public void reduce(){
        value /= 2;
    }
}
