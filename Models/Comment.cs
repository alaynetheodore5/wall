using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWall.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage="Message is required")]
        [MinLength(2)]
        public string CContent { get; set; }

        public int MessageId { get; set; }
        public int UserId { get; set; }

        public Message Maker { get; set; }
        public User Writer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
