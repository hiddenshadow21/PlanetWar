<?php

$email = "";
$code = "";

function DownloadDataFromUrl()
{
	global $email, $code;

	if(isset($_GET['email']))
	{
		$email=$_GET['email'];
	}
	else
	{
		return("V_DDFU_1");
	}
	
	if(isset($_GET['code']))
	{
		$code=$_GET['code'];
	}
	else
	{
		return("V_DDFU_2");
	}

	return("_SUCCESSFUL");
}

function VerifyData()
{
	global $email, $code;
	require "/var/www/logreg/_init.php";

	$request = "SELECT verified, code FROM user WHERE email = '".$email."';";
	$results = mysqli_query($CONNECTION, $request);
	
	if(mysqli_num_rows($results) == 1)
	{
		while($requestLoop = mysqli_fetch_array($results))
		{
			if(strcmp($requestLoop['code'], $code) == 0
			&& strcmp($requestLoop['verified'], "0") == 0)
			{
				return("_SUCCESSFUL");
			}
		}
		return("V_VD_1"); 
	}
	return("V_VD_2");
}

function UpdateData()
{
	global $email;
	require "/var/www/logreg/_init.php";

	$inputQuery = "UPDATE user SET verified='1' WHERE email='".$email."';";

	if(mysqli_query($CONNECTION, $inputQuery))
	{
		return("V_UD_SUCCESSFUL");
	}
	return("V_UD_1");

}

?>
