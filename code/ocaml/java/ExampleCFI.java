public class ExampleCFI
{
	private static int flag = 0;
	
    public static void main(String[] args) {
        try
        {
		  ExampleCFI hw = new ExampleCFI();
        }catch (Exception ex)
        {

        }
    }

    public ExampleCFI() throws Exception
    {
		// CFI START
        processVerifyPIN();
		
		if(flag != 5)
		{
			throw new Exception();
		}
    }

    private void processVerifyPIN() throws Exception
    {
		flag++;
		
        int pinLength = 4;
        int faultCode = 255;
        int triesRemaining;

        short count = setIncomingAndReceive();    // get expected data
		
		flag++;
		
        if(count < pinLength) throw new Exception();

        if(isInvalid() != false)
        {
            triesRemaining = getTriesRemaining();
            throw new Exception();
        }
		flag++;
    }


    private boolean isInvalid()
    {
		flag++;
        return true;
    }

    private short setIncomingAndReceive()
    {
		flag++;
        return 5;
    }

    private int getTriesRemaining()
    {
        return 2;
    }


}

