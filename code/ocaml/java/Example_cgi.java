public class Example_cgi
{
    private static int callId;

    public static void main(String[] args) {
        try
        {
            callId = 1;
		    Example hw = new Example();

        if(!(callId == 2))
        {
            throw new Exception();
        }

        }catch (Exception ex)
        {

        }
    }

    public Example_cgi() throws Exception
    {
        if(callId != 1)
        {
            throw new Exception();
        }

        callId = 2;

        processVerifyPIN();

        if(callId != 3)
        {
            throw new Exception();
        }

        callId = 2;
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

        callId = 4;

        if(isInvalid() != false)
        {
            if(callId != 5)
            {
                throw new Exception();
            }

            callId = 5;
            triesRemaining = getTriesRemaining();
            
            if(callId != 6)
            {
                throw new Exception();
            }

            throw new Exception();
        }

        callId = 2;
    }


    private boolean isInvalid()
    {
        if(callId != 4)
        {
            throw new Exception();
        }

        callId = 5;

        return true;
    }

    private short setIncomingAndReceive()
    {
        if(callId != 3)
        {
            throw new Exception();
        }

        callId = 4;
        return 5;
    }

    private int getTriesRemaining()
    {
        if(callId != 5)
        {
            throw new Exception();
        }

        callId = 6;

        return 2;
    }


}

