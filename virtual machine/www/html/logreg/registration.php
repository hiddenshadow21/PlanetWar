<?php

require "/var/www/logreg/_registration.php";

$response = DownloadDataFromUnity();

if($response == "_SUCCESSFUL")
{
	$responseTest[0] = UsernameTest();
	$responseTest[1] = EmailTest();
	$responseTest[2] = PasswordTest();

	for($i=0; $i<3; $i++)
	{
		if($responseTest[$i] != "_SUCCESSFUL")
		{
			//CloseConnection();
			exit($responseTest[$i]);
		}
	}
	
	$response = CreateNewAccount();
		
	if($response == "_SUCCESSFUL")
	{
		$response = SendVerificationMail();
		
		if($response != "R_CNA_SUCCESSFUL")
		{
			$response = DeleteNewAccount();
		}
	}
}

mysqli_close($CONNECTION);
exit($response);

?>
