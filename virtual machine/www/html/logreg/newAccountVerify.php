<?php

require "/var/www/logreg/_newAccountVerify.php";

$response = DownloadDataFromUrl();
if($response == "_SUCCESSFUL")
{
	$response = VerifyData();
	if($response == "_SUCCESSFUL")
	{
		$response = UpdateData();
		if($response == "V_UD_SUCCESSFUL")
		{
			echo "Your account has been activated.</br>You can now use it fully.";
			return;
		}
	}
}

echo "Registration error code: ".$response;

?>
