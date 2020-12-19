<?php

require "/var/www/logreg/_login.php";

$response = DownloadDataFromUnity();

if($response == "_SUCCESSFUL")
{
	$response = AuthorizeUser();
}

mysqli_close($CONNECTION);
exit($response);

?>
