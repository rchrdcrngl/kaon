package com.kaon.srvcregistry;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.netflix.eureka.server.EnableEurekaServer;

@SpringBootApplication
@EnableEurekaServer
public class SrvcregistryApplication {

	public static void main(String[] args) {
		SpringApplication.run(SrvcregistryApplication.class, args);
	}

}
