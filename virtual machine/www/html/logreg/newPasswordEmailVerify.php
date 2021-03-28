<?php

require "/var/www/logreg/_newPasswordEmailVerify.php";

$response = DownloadDataFromUnity();

if($response == "_SUCCESSFUL")
{
	$response = SetCode();
		
	if($response == "_SUCCESSFUL")
	{
		$response = SendVerificationMail();
	}
}

exit($response);

?>
