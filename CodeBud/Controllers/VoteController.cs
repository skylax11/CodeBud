using CodeBud.DbContext;
using CodeBud.Helpers;
using CodeBud.Models.Entities;
using CodeBud.Models.Repository.Relations;
using System.Linq;
using System.Web.Mvc;

namespace CodeBud.Controllers
{
    public class VoteController : Controller
    {
        private readonly SessionService.SessionService _sessionService = new SessionService.SessionService();

        [HttpPost]
        public ActionResult QuestionVote(int questionId, bool isUpvote)
        {
            var user = JwtHelper.GetCurrentUserFromToken();
            if (user == null)
                return Json(new { success = false, message = "Oturum açmalısınız." });

            using (var db = new AppDbContext())
            {
                var existingVote = db.QuestionVote
                    .Where(m => m.QuestionId == questionId)
                    .Join(db.Votes, m => m.VoteId, v => v.Id, (m, v) => new { Vote = v, Match = m })
                    .FirstOrDefault(x => x.Vote.UserId == user.Id);

                int voteChange = 0;

                if (existingVote != null)
                {
                    var vote = existingVote.Vote;

                    if (vote.IsUpvote == isUpvote)
                    {
                        // Oy kaldır
                        db.Votes.Remove(vote);
                        db.QuestionVote.Remove(existingVote.Match);
                        voteChange = isUpvote ? -1 : +1;
                    }
                    else
                    {
                        vote.IsUpvote = isUpvote;
                        voteChange = isUpvote ? +2 : -2;
                    }
                }
                else
                {
                    var vote = new Vote
                    {
                        UserId = user.Id,
                        IsUpvote = isUpvote,
                        QuestionId = questionId
                    };

                    db.Votes.Add(vote);
                    db.SaveChanges();

                    db.QuestionVote.Add(new QuestionVoteMatch
                    {
                        QuestionId = questionId,
                        VoteId = vote.Id
                    });

                    voteChange = isUpvote ? +1 : -1;
                }

                // VoteCount güncelle
                var question = db.Questions.Find(questionId);
                question.VoteCount += voteChange;

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    newCount = question.VoteCount,
                    voteStatus = (existingVote == null) ? (isUpvote ? "up" : "down") : "none"
                });
            }
        }
    }
}
