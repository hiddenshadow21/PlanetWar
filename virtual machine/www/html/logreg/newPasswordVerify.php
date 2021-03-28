<?php

require "/var/www/logreg/_newPasswordVerify.php";

$response = DownloadDataFromUnity();

if($response == "_SUCCESSFUL")
{
	$response = SetPassword();
}

exit($response);

?>
