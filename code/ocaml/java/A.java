public class A{
    protected short value = 10;
    
    public short bar(){
        return value;
    }
    
    public short foo(short v){
        value += v;        
        return value;
    }
}
