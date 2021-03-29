<?php

require "/var/www/logreg/_newPasswordCodeVerify.php";

$response = DownloadDataFromUnity();
if($response == "_SUCCESSFUL")
{
	$response = VerifyCode();
}

exit($response);

?>
