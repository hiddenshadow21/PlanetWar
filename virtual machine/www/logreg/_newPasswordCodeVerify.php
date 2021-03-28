<?php

$email = "";
$code = "";

function DownloadDataFromUnity()
{
	global $code, $email; 

	if(isset($_POST["code"]))
	{
		$code = $_POST["code"];
	}
	else
	{
		return("NPCV_DDFU_1");
	}

	if(isset($_POST["email"]))
	{
		$email = $_POST["email"];
	}
	else
	{
		return("NPCV_DDFU_2");
	}
	
	return("_SUCCESSFUL");
}

function VerifyCode()
{
	require "/var/www/logreg/_init.php";
	global $email, $code;

	$emailRequest = "SELECT code FROM user WHERE email = '".$email."' AND code = '".$code."';";
	$results = mysqli_query($CONNECTION, $emailRequest);

	if($results == false)
	{
		mysqli_close($CONNECTION);
		return("NPCV_VC_1");
	}

	if(mysqli_num_rows($results) == 1)
	{
		mysqli_close($CONNECTION);
		return("NPCV_VC_SUCCESSFUL");
	} 

	mysqli_close($CONNECTION);
	return("NPCV_VC_2");
}

?>
