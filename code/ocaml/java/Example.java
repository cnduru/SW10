public class Example 
{
    public static void main(String[] args) {
        try
        {
		  Example hw = new Example();
        }catch (Exception ex)
        {

        }
    }

    public Example() throws Exception
    {
        processVerifyPIN();
    }

    private void processVerifyPIN() throws Exception
    {
        int pinLength = 4;
        int faultCode = 255;
        int triesRemaining;

        short count = setIncomingAndReceive();    // get expected data

        if(count < pinLength) throw new Exception();

        if(check() == 0)
        {
            triesRemaining = getTriesRemaining();
            throw new Exception();
        }
    }


    private int check()
    {
        return 0;
    }

    private short setIncomingAndReceive()
    {
        return 5;
    }

    private int getTriesRemaining()
    {
        return 2;
    }


}

