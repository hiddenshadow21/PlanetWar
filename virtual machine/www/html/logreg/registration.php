<?php

require "/var/www/logreg/_registration.php";

$response = DownloadDataFromUnity();
if($response == "_SUCCESSFUL")
{
	$response = CreateNewAccount();
	if($response == "_SUCCESSFUL")
	{
		$response = SendVerificationMail();
		if($response != "R_SVM_SUCCESSFUL")
		{
			$response = DeleteNewAccount();
		}
	}
}

exit($response);

?>
