<?php
	use PHPMailer\PHPMailer\PHPMailer;
	use PHPMailer\PHPMailer\Exception;
	require '/var/www/mail/PHPMailer/src/Exception.php';
	require '/var/www/mail/PHPMailer/src/PHPMailer.php';
	require '/var/www/mail/PHPMailer/src/SMTP.php';

	function SendMail($email, $subject, $body)
	{
		$mail = new PHPMailer();
		$mail->isSMTP();
		$mail->SMTPAuth = true;
		$mail->SMTPSecure = 'ssl';
		$mail->Host = 'smtp.gmail.com';
		$mail->Port = '465';
		$mail->isHTML();
		$mail->Username = 'game.planetwar@gmail.com';
		$mail->Password = 'progzespBDJK20/21';
		$mail->SetFrom('game.planetwar@gmail.com','PlanetWar');
		$mail->Subject = $subject;
		$mail->Body = nl2br($body);
		$mail->AddAddress($email);
		$result = $mail->Send();

		if($result == 1)
		{
			return ("SM_SM_SUCCESSFUL_1");
		}
		return ("SM_SM_1");
	}
?>
