<?php

function SendAuthenticationMail($username, $email, $code)
{
	require "/var/www/mail/sendMail.php";

	$subject = "PlanetWar - new password verification";
	$body = "Hi ".$username.".";
	$body .= "\r\n Copy code below and paste it in game form!";
	$body .= "\r\n\r\n".$code."";
	$body .= "\r\n\r\n\r\n If you did not want to change your password, please ignore this message.";
	
	$response = SendMail($email, $subject, $body);
	
	return($response);
}

?>
