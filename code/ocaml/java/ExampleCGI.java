public class ExampleCGI
{
    private static int callId;

    public static void main(String[] args) {
        try
        {
            callId = 1;
		    ExampleCGI hw = new ExampleCGI();

        if(!(callId == 10))
        {
            throw new Exception();
        }

        }catch (Exception ex)
        {

        }
    }

    public ExampleCGI() throws Exception
    {
        if(callId != 1)
        {
            throw new Exception();
        }

        callId = 2;

        processVerifyPIN();

        if(callId != 9)
        {
            throw new Exception();
        }

        callId = 10;
    }

    private void processVerifyPIN() throws Exception
    {
        if(callId != 2)
        {
            throw new Exception();
        }

        int pinLength = 4;
        int faultCode = 255;
        int triesRemaining;

        callId = 3;

        short count = setIncomingAndReceive();    // get expected data

        if(callId != 4)
        {
            throw new Exception();
        }

        if(count < pinLength) throw new Exception();

        callId = 5;

        if(isInvalid() != false)
        {
            if(callId != 6)
            {
                throw new Exception();
            }

            callId = 7;
            triesRemaining = getTriesRemaining();
            
            if(callId != 8)
            {
                throw new Exception();
            }

            throw new Exception();
        }

        callId = 9;
    }


    private boolean isInvalid() throws Exception
    {
        if(callId != 5)
        {
            throw new Exception();
        }

        callId = 6;

        return true;
    }

    private short setIncomingAndReceive() throws Exception
    {
        if(callId != 3)
        {
            throw new Exception();
        }

        callId = 4;
        return 5;
    }

    private int getTriesRemaining() throws Exception
    {
        if(callId != 7)
        {
            throw new Exception();
        }

        callId = 8;

        return 2;
    }


}

