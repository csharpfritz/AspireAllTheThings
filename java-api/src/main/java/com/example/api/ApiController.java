package com.example.api;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import java.util.Map;

/**
 * Java Spring Boot API Controller for Aspire Demo
 * 
 * This is a simple REST API that demonstrates Aspire's Java support.
 * It exposes HTTP endpoints similar to the Python and Node.js demos.
 */
@RestController
public class ApiController {

    @GetMapping("/")
    public Map<String, String> home() {
        return Map.of(
            "message", "Hello from Java Spring Boot API!",
            "managed_by", "Aspire",
            "conference", "Swetugg Stockholm 2026"
        );
    }

    @GetMapping("/health")
    public Map<String, String> health() {
        return Map.of("status", "healthy");
    }

    @GetMapping("/info")
    public Map<String, String> info() {
        return Map.of(
            "runtime", "Java " + System.getProperty("java.version"),
            "framework", "Spring Boot",
            "vendor", System.getProperty("java.vendor")
        );
    }
}
